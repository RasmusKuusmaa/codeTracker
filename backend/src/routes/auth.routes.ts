import { FastifyInstance } from 'fastify';
import userController from '../controllers/user.controller';

async function authRoutes(fastify: FastifyInstance) {
    // Register new user (public route)
    fastify.post('/register', userController.register.bind(userController));

    // Login user (public route)
    fastify.post('/login', userController.login.bind(userController));
}

export default authRoutes;
