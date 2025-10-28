import { FastifyInstance } from 'fastify';
import userRoutes from './user.routes';
import sessionRoutes from './session.routes';

async function routes(fastify: FastifyInstance) {
    fastify.get('/health', async (request, reply) => {
        return {
            status: 'ok',
            timestamp: new Date().toISOString(),
            uptime: process.uptime()
        };
    });

    await fastify.register(userRoutes);
    await fastify.register(sessionRoutes);
}

export default routes;
