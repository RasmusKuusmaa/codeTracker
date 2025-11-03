import sessionController from "@controllers/session.controller";
import { authenticate } from "@middleware/auth";
import { FastifyInstance } from "fastify";

async function sessionRoutes(fastify: FastifyInstance) {
    fastify.get('/sessions', { preHandler: authenticate }, sessionController.getAllSessions);
    fastify.get('/sessions/active', { preHandler: authenticate }, sessionController.getActiveSession);
    fastify.get('/sessions/last', { preHandler: authenticate }, sessionController.getLastSession);
    fastify.get('/sessions/:id', { preHandler: authenticate }, sessionController.getSessionById);
    fastify.post('/sessions', { preHandler: authenticate }, sessionController.createSession);
    fastify.put('/sessions/:id', { preHandler: authenticate }, sessionController.updateSession);
    fastify.delete('/sessions/:id', { preHandler: authenticate }, sessionController.deleteSession);
    fastify.post('/sessions/:id/pause', { preHandler: authenticate }, sessionController.pauseSession);
    fastify.post('/sessions/:id/resume', { preHandler: authenticate }, sessionController.resumeSession);
    fastify.post('/sessions/:id/stop', { preHandler: authenticate }, sessionController.stopSession);
    fastify.get('/sessions/project/:projectId', { preHandler: authenticate }, sessionController.getSessionsByProjectId);
    fastify.get('/sessions/language/:languageId', { preHandler: authenticate }, sessionController.getSessionsByLanguageId);
}

export default sessionRoutes;