import database from "@config/database";
import { AppError } from "../types/common.types";
import { SessionRow } from "../types/session.types";


export class SessionModel {
    private pool = database.getPool();

    async findAllSessions(): Promise<SessionRow[]> {
        try {
                const [rows] = await this.pool.query<SessionRow[]>(
                    `SELECT session_id,
                        user_id, 
                        time_started,
                        time_ended,
                        title from sessions; `
                );
                return rows;
        } catch (error) {
            throw new AppError(500, 'Error fetching sessions from database');
        }
    }
}