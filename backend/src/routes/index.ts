import { FastifyInstance } from 'fastify';
import authRoutes from './auth.routes';
import userRoutes from './user.routes';
import sessionRoutes from './session.routes';

async function routes(fastify: FastifyInstance) {
    // Register all routes under /api prefix
    await fastify.register(async (api) => {
        // Health check endpoint
        api.get('/health', async (request, reply) => {
            return {
                status: 'ok',
                timestamp: new Date().toISOString(),
                uptime: process.uptime()
            };
        });

        // Auth routes under /api/auth
        await api.register(authRoutes, { prefix: '/auth' });

        // User and session routes under /api/user
        await api.register(userRoutes, { prefix: '/user' });
        await api.register(sessionRoutes, { prefix: '/user' });
    }, { prefix: '/api' });
}

export default routes;
