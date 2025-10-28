import sessionService from "@services/session.service";
import { ApiResponse } from "../types/common.types";
import { logger } from "@utils/logger";
import { FastifyReply, FastifyRequest } from "fastify";


export class SessionController {
    async getAllSessions(request: FastifyRequest, reply: FastifyReply): Promise<void> {
        try {
            const userId = request.user!.user_id;
            const sessions = await sessionService.getAllSessions(userId);

            const response: ApiResponse = {
                success: true,
                data: sessions,
                message: 'Sessions Retrieved Succeessfully'
            };
            reply.code(200).send(response);
        } catch (error: any) {
            logger.error('Error in getAllSessions controller:', error);
            reply.code(error.statusCode || 500).send({
                success: false,
                error: error.message || 'Internal Server error'
            });
        }
    }
}

export default new SessionController();