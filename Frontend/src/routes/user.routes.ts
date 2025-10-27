import { FastifyInstance } from 'fastify';
import userController from '../controllers/user.controller';

async function userRoutes(fastify: FastifyInstance) {
    fastify.get('/users', userController.getAllUsers.bind(userController));
}

export default userRoutes;
