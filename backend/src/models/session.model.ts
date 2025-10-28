import database from "@config/database";
import { AppError } from "../types/common.types";
import { CsessionRow } from "../types/csession.types";


export class SessionModel {
    private pool = database.getPool();

    async findAllSessions(): Promise<CsessionRow[]> {
        try {
                const [rows] = await this.pool.query<CsessionRow[]>(
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

export default new SessionModel();