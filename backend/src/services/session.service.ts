import sessionModel from "@models/session.model";
import { Csession, CreateSessionDTO, UpdateSessionDTO, SessionStatus, SessionWithDetails } from "../types/csession.types";
import { logger } from "@utils/logger";
import { AppError } from "../types/common.types";

export class SessionService {
    async getAllSessions(userId: number): Promise<SessionWithDetails[]> {
        try {
            return await sessionModel.findAllSessions(userId);
        } catch (error) {
            logger.error('Error in getAllSessions service:', error);
            throw error;
        }
    }

    async getSessionById(sessionId: number, userId: number): Promise<SessionWithDetails> {
        try {
            const session = await sessionModel.findSessionById(sessionId, userId);
            if (!session) {
                throw new AppError(404, 'Session not found');
            }
            return session;
        } catch (error) {
            logger.error('Error in getSessionById service:', error);
            throw error;
        }
    }

    async getActiveSession(userId: number): Promise<SessionWithDetails | null> {
        try {
            return await sessionModel.findActiveSession(userId);
        } catch (error) {
            logger.error('Error in getActiveSession service:', error);
            throw error;
        }
    }

    async getLastSession(userId: number): Promise<SessionWithDetails | null> {
        try {
            return await sessionModel.findLastSession(userId);
        } catch (error) {
            logger.error('Error in getLastSession service:', error);
            throw error;
        }
    }

    async createSession(userId: number, sessionData: CreateSessionDTO): Promise<SessionWithDetails> {
        try {
            const activeSession = await sessionModel.findActiveSession(userId);
            if (activeSession) {
                throw new AppError(400, 'Cannot start a new session while another session is active');
            }

            const sessionId = await sessionModel.createSession(userId, sessionData);
            const session = await sessionModel.findSessionById(sessionId, userId);
            if (!session) {
                throw new AppError(500, 'Error retrieving created session');
            }
            return session;
        } catch (error) {
            logger.error('Error in createSession service:', error);
            throw error;
        }
    }

    async pauseSession(sessionId: number, userId: number, durationSeconds: number): Promise<SessionWithDetails> {
        try {
            const session = await sessionModel.findSessionById(sessionId, userId);
            if (!session) {
                throw new AppError(404, 'Session not found');
            }
            if (session.status !== 'running') {
                throw new AppError(400, 'Can only pause a running session');
            }

            const updated = await sessionModel.updateSessionStatus(sessionId, userId, 'paused', durationSeconds);
            if (!updated) {
                throw new AppError(500, 'Error pausing session');
            }

            const updatedSession = await sessionModel.findSessionById(sessionId, userId);
            if (!updatedSession) {
                throw new AppError(500, 'Error retrieving paused session');
            }
            return updatedSession;
        } catch (error) {
            logger.error('Error in pauseSession service:', error);
            throw error;
        }
    }

    async resumeSession(sessionId: number, userId: number): Promise<SessionWithDetails> {
        try {
            const session = await sessionModel.findSessionById(sessionId, userId);
            if (!session) {
                throw new AppError(404, 'Session not found');
            }
            if (session.status !== 'paused') {
                throw new AppError(400, 'Can only resume a paused session');
            }

            const updated = await sessionModel.updateSessionStatus(sessionId, userId, 'running');
            if (!updated) {
                throw new AppError(500, 'Error resuming session');
            }

            const updatedSession = await sessionModel.findSessionById(sessionId, userId);
            if (!updatedSession) {
                throw new AppError(500, 'Error retrieving resumed session');
            }
            return updatedSession;
        } catch (error) {
            logger.error('Error in resumeSession service:', error);
            throw error;
        }
    }

    async stopSession(sessionId: number, userId: number, durationSeconds: number): Promise<SessionWithDetails> {
        try {
            const session = await sessionModel.findSessionById(sessionId, userId);
            if (!session) {
                throw new AppError(404, 'Session not found');
            }
            if (session.status === 'stopped') {
                throw new AppError(400, 'Session is already stopped');
            }

            const updated = await sessionModel.updateSessionStatus(sessionId, userId, 'stopped', durationSeconds);
            if (!updated) {
                throw new AppError(500, 'Error stopping session');
            }

            const updatedSession = await sessionModel.findSessionById(sessionId, userId);
            if (!updatedSession) {
                throw new AppError(500, 'Error retrieving stopped session');
            }
            return updatedSession;
        } catch (error) {
            logger.error('Error in stopSession service:', error);
            throw error;
        }
    }

    async updateSession(sessionId: number, userId: number, sessionData: UpdateSessionDTO): Promise<SessionWithDetails> {
        try {
            const session = await sessionModel.findSessionById(sessionId, userId);
            if (!session) {
                throw new AppError(404, 'Session not found');
            }

            const updated = await sessionModel.updateSession(sessionId, userId, sessionData);
            if (!updated) {
                throw new AppError(404, 'Session not found or no changes made');
            }

            const updatedSession = await sessionModel.findSessionById(sessionId, userId);
            if (!updatedSession) {
                throw new AppError(500, 'Error retrieving updated session');
            }
            return updatedSession;
        } catch (error) {
            logger.error('Error in updateSession service:', error);
            throw error;
        }
    }

    async deleteSession(sessionId: number, userId: number): Promise<void> {
        try {
            const deleted = await sessionModel.deleteSession(sessionId, userId);
            if (!deleted) {
                throw new AppError(404, 'Session not found');
            }
        } catch (error) {
            logger.error('Error in deleteSession service:', error);
            throw error;
        }
    }

    async getSessionsByProjectId(projectId: number, userId: number): Promise<SessionWithDetails[]> {
        try {
            return await sessionModel.getSessionsByProjectId(projectId, userId);
        } catch (error) {
            logger.error('Error in getSessionsByProjectId service:', error);
            throw error;
        }
    }

    async getSessionsByLanguageId(languageId: number, userId: number): Promise<SessionWithDetails[]> {
        try {
            return await sessionModel.getSessionsByLanguageId(languageId, userId);
        } catch (error) {
            logger.error('Error in getSessionsByLanguageId service:', error);
            throw error;
        }
    }
}

export default new SessionService();