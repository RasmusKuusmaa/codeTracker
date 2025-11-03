import database from "@config/database";
import { AppError } from "../types/common.types";
import { CsessionRow, Csession, CreateSessionDTO, UpdateSessionDTO, Language, SessionStatus, SessionWithDetails } from "../types/csession.types";

export class SessionModel {
    private pool = database.getPool();

    async findAllSessions(userId: number): Promise<SessionWithDetails[]> {
        try {
            const [rows] = await this.pool.query<any[]>(
                `SELECT s.session_id, s.user_id, s.project_id, s.time_started, s.time_ended,
                        s.title, s.status, s.duration_seconds, p.name as project_name
                FROM sessions s
                LEFT JOIN projects p ON s.project_id = p.project_id
                WHERE s.user_id = ?
                ORDER BY s.time_started DESC`,
                [userId]
            );

            const sessions = await Promise.all(rows.map(async (row) => {
                const languages = await this.getSessionLanguages(row.session_id);
                return {
                    ...row,
                    languages
                };
            }));

            return sessions;
        } catch (error) {
            throw new AppError(500, 'Error fetching sessions from database');
        }
    }

    async findSessionById(sessionId: number, userId: number): Promise<SessionWithDetails | null> {
        try {
            const [rows] = await this.pool.query<any[]>(
                `SELECT s.session_id, s.user_id, s.project_id, s.time_started, s.time_ended,
                        s.title, s.status, s.duration_seconds, p.name as project_name
                FROM sessions s
                LEFT JOIN projects p ON s.project_id = p.project_id
                WHERE s.session_id = ? AND s.user_id = ?`,
                [sessionId, userId]
            );

            if (rows.length === 0) return null;

            const languages = await this.getSessionLanguages(sessionId);
            return {
                ...rows[0],
                languages
            };
        } catch (error) {
            throw new AppError(500, 'Error fetching session from database');
        }
    }

    async findActiveSession(userId: number): Promise<SessionWithDetails | null> {
        try {
            const [rows] = await this.pool.query<any[]>(
                `SELECT s.session_id, s.user_id, s.project_id, s.time_started, s.time_ended,
                        s.title, s.status, s.duration_seconds, p.name as project_name
                FROM sessions s
                LEFT JOIN projects p ON s.project_id = p.project_id
                WHERE s.user_id = ? AND s.status IN ('running', 'paused')
                ORDER BY s.time_started DESC
                LIMIT 1`,
                [userId]
            );

            if (rows.length === 0) return null;

            const languages = await this.getSessionLanguages(rows[0].session_id);
            return {
                ...rows[0],
                languages
            };
        } catch (error) {
            throw new AppError(500, 'Error fetching active session from database');
        }
    }

    async findLastSession(userId: number): Promise<SessionWithDetails | null> {
        try {
            const [rows] = await this.pool.query<any[]>(
                `SELECT s.session_id, s.user_id, s.project_id, s.time_started, s.time_ended,
                        s.title, s.status, s.duration_seconds, p.name as project_name
                FROM sessions s
                LEFT JOIN projects p ON s.project_id = p.project_id
                WHERE s.user_id = ?
                ORDER BY s.time_started DESC
                LIMIT 1`,
                [userId]
            );

            if (rows.length === 0) return null;

            const languages = await this.getSessionLanguages(rows[0].session_id);
            return {
                ...rows[0],
                languages
            };
        } catch (error) {
            throw new AppError(500, 'Error fetching last session from database');
        }
    }

    async createSession(userId: number, sessionData: CreateSessionDTO): Promise<number> {
        const connection = await this.pool.getConnection();
        try {
            await connection.beginTransaction();

            const [result] = await connection.query(
                `INSERT INTO sessions (user_id, project_id, title, time_started, status, duration_seconds)
                VALUES (?, ?, ?, NOW(), 'running', 0)`,
                [userId, sessionData.project_id || null, sessionData.title]
            );

            const sessionId = (result as any).insertId;

            if (sessionData.language_ids && sessionData.language_ids.length > 0) {
                await this.addSessionLanguages(connection, sessionId, sessionData.language_ids);
            }

            await connection.commit();
            return sessionId;
        } catch (error) {
            await connection.rollback();
            throw new AppError(500, 'Error creating session in database');
        } finally {
            connection.release();
        }
    }

    async updateSessionStatus(sessionId: number, userId: number, status: SessionStatus, durationSeconds?: number): Promise<boolean> {
        try {
            const updates: string[] = ['status = ?'];
            const values: any[] = [status];

            if (status === 'stopped') {
                updates.push('time_ended = NOW()');
            }

            if (durationSeconds !== undefined) {
                updates.push('duration_seconds = ?');
                values.push(durationSeconds);
            }

            values.push(sessionId, userId);

            const [result] = await this.pool.query(
                `UPDATE sessions SET ${updates.join(', ')} WHERE session_id = ? AND user_id = ?`,
                values
            );

            const affected = (result as any).affectedRows > 0;

            if (affected && status === 'stopped') {
                await this.updateProjectTotalTime(sessionId);
            }

            return affected;
        } catch (error) {
            throw new AppError(500, 'Error updating session status in database');
        }
    }

    async updateSession(sessionId: number, userId: number, sessionData: UpdateSessionDTO): Promise<boolean> {
        const connection = await this.pool.getConnection();
        try {
            await connection.beginTransaction();

            const fields: string[] = [];
            const values: any[] = [];

            if (sessionData.title !== undefined) {
                fields.push('title = ?');
                values.push(sessionData.title);
            }
            if (sessionData.project_id !== undefined) {
                fields.push('project_id = ?');
                values.push(sessionData.project_id);
            }

            if (fields.length > 0) {
                values.push(sessionId, userId);
                const [result] = await connection.query(
                    `UPDATE sessions SET ${fields.join(', ')} WHERE session_id = ? AND user_id = ?`,
                    values
                );

                if ((result as any).affectedRows === 0) {
                    await connection.rollback();
                    return false;
                }
            }

            if (sessionData.language_ids !== undefined) {
                await connection.query('DELETE FROM session_languages WHERE session_id = ?', [sessionId]);
                if (sessionData.language_ids.length > 0) {
                    await this.addSessionLanguages(connection, sessionId, sessionData.language_ids);
                }
            }

            await connection.commit();
            return true;
        } catch (error) {
            await connection.rollback();
            throw new AppError(500, 'Error updating session in database');
        } finally {
            connection.release();
        }
    }

    async deleteSession(sessionId: number, userId: number): Promise<boolean> {
        const connection = await this.pool.getConnection();
        try {
            await connection.beginTransaction();

            await connection.query('DELETE FROM session_languages WHERE session_id = ?', [sessionId]);

            const [result] = await connection.query(
                'DELETE FROM sessions WHERE session_id = ? AND user_id = ?',
                [sessionId, userId]
            );

            await connection.commit();
            return (result as any).affectedRows > 0;
        } catch (error) {
            await connection.rollback();
            throw new AppError(500, 'Error deleting session from database');
        } finally {
            connection.release();
        }
    }

    async getSessionLanguages(sessionId: number): Promise<Language[]> {
        try {
            const [rows] = await this.pool.query<any[]>(
                `SELECT l.language_id, l.name
                FROM languages l
                INNER JOIN session_languages sl ON l.language_id = sl.language_id
                WHERE sl.session_id = ?`,
                [sessionId]
            );
            return rows;
        } catch (error) {
            throw new AppError(500, 'Error fetching session languages from database');
        }
    }

    async getSessionsByProjectId(projectId: number, userId: number): Promise<SessionWithDetails[]> {
        try {
            const [rows] = await this.pool.query<any[]>(
                `SELECT s.session_id, s.user_id, s.project_id, s.time_started, s.time_ended,
                        s.title, s.status, s.duration_seconds, p.name as project_name
                FROM sessions s
                LEFT JOIN projects p ON s.project_id = p.project_id
                WHERE s.project_id = ? AND s.user_id = ?
                ORDER BY s.time_started DESC`,
                [projectId, userId]
            );

            const sessions = await Promise.all(rows.map(async (row) => {
                const languages = await this.getSessionLanguages(row.session_id);
                return {
                    ...row,
                    languages
                };
            }));

            return sessions;
        } catch (error) {
            throw new AppError(500, 'Error fetching sessions by project from database');
        }
    }

    async getSessionsByLanguageId(languageId: number, userId: number): Promise<SessionWithDetails[]> {
        try {
            const [rows] = await this.pool.query<any[]>(
                `SELECT s.session_id, s.user_id, s.project_id, s.time_started, s.time_ended,
                        s.title, s.status, s.duration_seconds, p.name as project_name
                FROM sessions s
                LEFT JOIN projects p ON s.project_id = p.project_id
                INNER JOIN session_languages sl ON s.session_id = sl.session_id
                WHERE sl.language_id = ? AND s.user_id = ?
                ORDER BY s.time_started DESC`,
                [languageId, userId]
            );

            const sessions = await Promise.all(rows.map(async (row) => {
                const languages = await this.getSessionLanguages(row.session_id);
                return {
                    ...row,
                    languages
                };
            }));

            return sessions;
        } catch (error) {
            throw new AppError(500, 'Error fetching sessions by language from database');
        }
    }

    private async addSessionLanguages(connection: any, sessionId: number, languageIds: number[]): Promise<void> {
        const values = languageIds.map(langId => [sessionId, langId]);
        await connection.query(
            'INSERT INTO session_languages (session_id, language_id) VALUES ?',
            [values]
        );
    }

    private async updateProjectTotalTime(sessionId: number): Promise<void> {
        try {
            await this.pool.query(
                `UPDATE projects p
                SET total_time_minutes = (
                    SELECT COALESCE(SUM(CEIL(s.duration_seconds / 60)), 0)
                    FROM sessions s
                    WHERE s.project_id = p.project_id AND s.status = 'stopped'
                )
                WHERE p.project_id = (
                    SELECT project_id FROM sessions WHERE session_id = ?
                )`,
                [sessionId]
            );
        } catch (error) {
            throw new AppError(500, 'Error updating project total time');
        }
    }
}

export default new SessionModel();