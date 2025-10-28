import database from "@config/database";
import { AppError } from "../types/common.types";
import { CsessionRow } from "../types/csession.types";


export class SessionModel {
    private pool = database.getPool();

    async findAllSessions(userId: number): Promise<CsessionRow[]> {
        try {
                const [rows] = await this.pool.query<CsessionRow[]>(
                    `SELECT session_id,
                        user_id,
                        time_started,
                        time_ended,
                        title
                    FROM sessions
                    WHERE user_id = ?`,
                    [userId]
                );
                return rows;
        } catch (error) {
            throw new AppError(500, 'Error fetching sessions from database');
        }
    }
}

export default new SessionModel();