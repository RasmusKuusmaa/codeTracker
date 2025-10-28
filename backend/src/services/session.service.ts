import sessionModel, { SessionModel } from "@models/session.model";
import { Csession } from "../types/csession.types";
import { logger } from "@utils/logger";


export class SessionService {
    async getAllSessions(userId: number): Promise<Csession[]> {
        try {
            const sessions = await sessionModel.findAllSessions(userId);
            return sessions.map(session => ({
                session_id: session.session_id,
                user_id: session.user_id,
                time_started: session.time_started,
                time_ended: session.time_ended,
                title: session.title
            }));
        } catch (error) {
            logger.error('Error in getAllSessions service;', error);
            throw error;
        }
    }
}

export default new SessionService();