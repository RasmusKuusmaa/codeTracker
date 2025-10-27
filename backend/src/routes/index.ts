import { FastifyInstance } from 'fastify';
import userRoutes from './user.routes';

async function routes(fastify: FastifyInstance) {
    fastify.get('/health', async (request, reply) => {
        return {
            status: 'ok',
            timestamp: new Date().toISOString(),
            uptime: process.uptime()
        };
    });

    await fastify.register(userRoutes);
}

export default routes;
