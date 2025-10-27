import { FastifyInstance } from 'fastify';
import userController from '../controllers/user.controller';

async function userRoutes(fastify: FastifyInstance) {
    // Get all users
    fastify.get('/users', userController.getAllUsers.bind(userController));

    // Register new user
    fastify.post('/register', userController.register.bind(userController));

    // Login user
    fastify.post('/login', userController.login.bind(userController));
}

export default userRoutes;
