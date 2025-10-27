import { FastifyRequest, FastifyReply } from 'fastify';
import userService from '../services/user.service';
import { ApiResponse } from '../types/common.types';
import { logger } from '../utils/logger';

export class UserController {
    async getAllUsers(request: FastifyRequest, reply: FastifyReply): Promise<void> {
        try {
            const users = await userService.getAllUsers();

            const response: ApiResponse = {
                success: true,
                data: users,
                message: 'Users retrieved successfully'
            };

            reply.code(200).send(response);
        } catch (error: any) {
            logger.error('Error in getAllUsers controller:', error);
            reply.code(error.statusCode || 500).send({
                success: false,
                error: error.message || 'Internal server error'
            });
        }
    }
}

export default new UserController();
