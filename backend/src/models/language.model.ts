import database from "@config/database";
import { AppError } from "../types/common.types";
import { Language, LanguageRow } from "../types/csession.types";

export class LanguageModel {
    private pool = database.getPool();

    async findAllLanguages(): Promise<Language[]> {
        try {
            const [rows] = await this.pool.query<LanguageRow[]>(
                'SELECT language_id, name FROM languages ORDER BY name'
            );
            return rows;
        } catch (error) {
            throw new AppError(500, 'Error fetching languages from database');
        }
    }

    async findLanguageById(languageId: number): Promise<Language | null> {
        try {
            const [rows] = await this.pool.query<LanguageRow[]>(
                'SELECT language_id, name FROM languages WHERE language_id = ?',
                [languageId]
            );
            return rows[0] || null;
        } catch (error) {
            throw new AppError(500, 'Error fetching language from database');
        }
    }

    async findLanguageByName(name: string): Promise<Language | null> {
        try {
            const [rows] = await this.pool.query<LanguageRow[]>(
                'SELECT language_id, name FROM languages WHERE name = ?',
                [name]
            );
            return rows[0] || null;
        } catch (error) {
            throw new AppError(500, 'Error fetching language from database');
        }
    }

    async createLanguage(name: string): Promise<number> {
        try {
            const existing = await this.findLanguageByName(name);
            if (existing) {
                throw new AppError(409, 'Language already exists');
            }

            const [result] = await this.pool.query(
                'INSERT INTO languages (name) VALUES (?)',
                [name]
            );
            return (result as any).insertId;
        } catch (error: any) {
            if (error instanceof AppError) throw error;
            throw new AppError(500, 'Error creating language in database');
        }
    }

    async getLanguageStats(userId: number): Promise<any[]> {
        try {
            const [rows] = await this.pool.query<any[]>(
                `SELECT l.language_id, l.name,
                    COUNT(DISTINCT s.session_id) as session_count,
                    COALESCE(SUM(CASE WHEN s.status = 'stopped' THEN s.duration_seconds ELSE 0 END), 0) as total_seconds
                FROM languages l
                LEFT JOIN session_languages sl ON l.language_id = sl.language_id
                LEFT JOIN sessions s ON sl.session_id = s.session_id AND s.user_id = ?
                GROUP BY l.language_id, l.name
                HAVING session_count > 0
                ORDER BY total_seconds DESC`,
                [userId]
            );
            return rows;
        } catch (error) {
            throw new AppError(500, 'Error fetching language stats from database');
        }
    }
}

export default new LanguageModel();
