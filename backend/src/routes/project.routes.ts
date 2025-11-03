import projectController from "@controllers/project.controller";
import { authenticate } from "@middleware/auth";
import { FastifyInstance } from "fastify";

async function projectRoutes(fastify: FastifyInstance) {
    fastify.get('/projects', { preHandler: authenticate }, projectController.getAllProjects);
    fastify.get('/projects/:id', { preHandler: authenticate }, projectController.getProjectById);
    fastify.post('/projects', { preHandler: authenticate }, projectController.createProject);
    fastify.put('/projects/:id', { preHandler: authenticate }, projectController.updateProject);
    fastify.delete('/projects/:id', { preHandler: authenticate }, projectController.deleteProject);
}

export default projectRoutes;
