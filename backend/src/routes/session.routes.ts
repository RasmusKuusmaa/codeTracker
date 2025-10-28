import sessionController from "@controllers/session.controller";
import { authenticate } from "@middleware/auth";
import { FastifyInstance } from "fastify";


async function sessionRoutes(fastify:FastifyInstance) {
    fastify.get('/sessions', {preHandler: authenticate}, sessionController.getAllSessions);
}
export default sessionRoutes;