import sessionService from "@services/session.service";
import { ApiResponse } from "../types/common.types";
import { logger } from "@utils/logger";
import { FastifyReply, FastifyRequest } from "fastify";
import { CreateSessionDTO, UpdateSessionDTO } from "../types/csession.types";

export class SessionController {
    async getAllSessions(request: FastifyRequest, reply: FastifyReply): Promise<void> {
        try {
            const userId = request.user!.user_id;
            const sessions = await sessionService.getAllSessions(userId);

            const response: ApiResponse = {
                success: true,
                data: sessions,
                message: 'Sessions retrieved successfully'
            };
            reply.code(200).send(response);
        } catch (error: any) {
            logger.error('Error in getAllSessions controller:', error);
            reply.code(error.statusCode || 500).send({
                success: false,
                error: error.message || 'Internal server error'
            });
        }
    }

    async getSessionById(request: FastifyRequest<{ Params: { id: string } }>, reply: FastifyReply): Promise<void> {
        try {
            const userId = request.user!.user_id;
            const sessionId = parseInt(request.params.id);
            const session = await sessionService.getSessionById(sessionId, userId);

            const response: ApiResponse = {
                success: true,
                data: session,
                message: 'Session retrieved successfully'
            };
            reply.code(200).send(response);
        } catch (error: any) {
            logger.error('Error in getSessionById controller:', error);
            reply.code(error.statusCode || 500).send({
                success: false,
                error: error.message || 'Internal server error'
            });
        }
    }

    async getActiveSession(request: FastifyRequest, reply: FastifyReply): Promise<void> {
        try {
            const userId = request.user!.user_id;
            const session = await sessionService.getActiveSession(userId);

            const response: ApiResponse = {
                success: true,
                data: session,
                message: 'Active session retrieved successfully'
            };
            reply.code(200).send(response);
        } catch (error: any) {
            logger.error('Error in getActiveSession controller:', error);
            reply.code(error.statusCode || 500).send({
                success: false,
                error: error.message || 'Internal server error'
            });
        }
    }

    async getLastSession(request: FastifyRequest, reply: FastifyReply): Promise<void> {
        try {
            const userId = request.user!.user_id;
            const session = await sessionService.getLastSession(userId);

            const response: ApiResponse = {
                success: true,
                data: session,
                message: 'Last session retrieved successfully'
            };
            reply.code(200).send(response);
        } catch (error: any) {
            logger.error('Error in getLastSession controller:', error);
            reply.code(error.statusCode || 500).send({
                success: false,
                error: error.message || 'Internal server error'
            });
        }
    }

    async createSession(request: FastifyRequest<{ Body: CreateSessionDTO }>, reply: FastifyReply): Promise<void> {
        try {
            const userId = request.user!.user_id;
            const session = await sessionService.createSession(userId, request.body);

            const response: ApiResponse = {
                success: true,
                data: session,
                message: 'Session created successfully'
            };
            reply.code(201).send(response);
        } catch (error: any) {
            logger.error('Error in createSession controller:', error);
            reply.code(error.statusCode || 500).send({
                success: false,
                error: error.message || 'Internal server error'
            });
        }
    }

    async pauseSession(request: FastifyRequest<{ Params: { id: string }, Body: { duration_seconds: number } }>, reply: FastifyReply): Promise<void> {
        try {
            const userId = request.user!.user_id;
            const sessionId = parseInt(request.params.id);
            const { duration_seconds } = request.body;

            const session = await sessionService.pauseSession(sessionId, userId, duration_seconds);

            const response: ApiResponse = {
                success: true,
                data: session,
                message: 'Session paused successfully'
            };
            reply.code(200).send(response);
        } catch (error: any) {
            logger.error('Error in pauseSession controller:', error);
            reply.code(error.statusCode || 500).send({
                success: false,
                error: error.message || 'Internal server error'
            });
        }
    }

    async resumeSession(request: FastifyRequest<{ Params: { id: string } }>, reply: FastifyReply): Promise<void> {
        try {
            const userId = request.user!.user_id;
            const sessionId = parseInt(request.params.id);

            const session = await sessionService.resumeSession(sessionId, userId);

            const response: ApiResponse = {
                success: true,
                data: session,
                message: 'Session resumed successfully'
            };
            reply.code(200).send(response);
        } catch (error: any) {
            logger.error('Error in resumeSession controller:', error);
            reply.code(error.statusCode || 500).send({
                success: false,
                error: error.message || 'Internal server error'
            });
        }
    }

    async stopSession(request: FastifyRequest<{ Params: { id: string }, Body: { duration_seconds: number } }>, reply: FastifyReply): Promise<void> {
        try {
            const userId = request.user!.user_id;
            const sessionId = parseInt(request.params.id);
            const { duration_seconds } = request.body;

            const session = await sessionService.stopSession(sessionId, userId, duration_seconds);

            const response: ApiResponse = {
                success: true,
                data: session,
                message: 'Session stopped successfully'
            };
            reply.code(200).send(response);
        } catch (error: any) {
            logger.error('Error in stopSession controller:', error);
            reply.code(error.statusCode || 500).send({
                success: false,
                error: error.message || 'Internal server error'
            });
        }
    }

    async updateSession(request: FastifyRequest<{ Params: { id: string }, Body: UpdateSessionDTO }>, reply: FastifyReply): Promise<void> {
        try {
            const userId = request.user!.user_id;
            const sessionId = parseInt(request.params.id);

            const session = await sessionService.updateSession(sessionId, userId, request.body);

            const response: ApiResponse = {
                success: true,
                data: session,
                message: 'Session updated successfully'
            };
            reply.code(200).send(response);
        } catch (error: any) {
            logger.error('Error in updateSession controller:', error);
            reply.code(error.statusCode || 500).send({
                success: false,
                error: error.message || 'Internal server error'
            });
        }
    }

    async deleteSession(request: FastifyRequest<{ Params: { id: string } }>, reply: FastifyReply): Promise<void> {
        try {
            const userId = request.user!.user_id;
            const sessionId = parseInt(request.params.id);

            await sessionService.deleteSession(sessionId, userId);

            const response: ApiResponse = {
                success: true,
                message: 'Session deleted successfully'
            };
            reply.code(200).send(response);
        } catch (error: any) {
            logger.error('Error in deleteSession controller:', error);
            reply.code(error.statusCode || 500).send({
                success: false,
                error: error.message || 'Internal server error'
            });
        }
    }

    async getSessionsByProjectId(request: FastifyRequest<{ Params: { projectId: string } }>, reply: FastifyReply): Promise<void> {
        try {
            const userId = request.user!.user_id;
            const projectId = parseInt(request.params.projectId);
            const sessions = await sessionService.getSessionsByProjectId(projectId, userId);

            const response: ApiResponse = {
                success: true,
                data: sessions,
                message: 'Sessions retrieved successfully'
            };
            reply.code(200).send(response);
        } catch (error: any) {
            logger.error('Error in getSessionsByProjectId controller:', error);
            reply.code(error.statusCode || 500).send({
                success: false,
                error: error.message || 'Internal server error'
            });
        }
    }

    async getSessionsByLanguageId(request: FastifyRequest<{ Params: { languageId: string } }>, reply: FastifyReply): Promise<void> {
        try {
            const userId = request.user!.user_id;
            const languageId = parseInt(request.params.languageId);
            const sessions = await sessionService.getSessionsByLanguageId(languageId, userId);

            const response: ApiResponse = {
                success: true,
                data: sessions,
                message: 'Sessions retrieved successfully'
            };
            reply.code(200).send(response);
        } catch (error: any) {
            logger.error('Error in getSessionsByLanguageId controller:', error);
            reply.code(error.statusCode || 500).send({
                success: false,
                error: error.message || 'Internal server error'
            });
        }
    }
}

export default new SessionController();