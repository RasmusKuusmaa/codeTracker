import { FastifyInstance } from 'fastify';
import userController from '../controllers/user.controller';
import { authenticate } from '../middleware/auth';

async function userRoutes(fastify: FastifyInstance) {
    // Get all users (protected route)
}

export default userRoutes;
