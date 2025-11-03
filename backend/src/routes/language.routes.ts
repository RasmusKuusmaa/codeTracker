import languageController from "@controllers/language.controller";
import { authenticate } from "@middleware/auth";
import { FastifyInstance } from "fastify";

async function languageRoutes(fastify: FastifyInstance) {
    fastify.get('/languages', languageController.getAllLanguages);
    fastify.get('/languages/stats', { preHandler: authenticate }, languageController.getLanguageStats);
    fastify.get('/languages/:id', languageController.getLanguageById);
    fastify.post('/languages', { preHandler: authenticate }, languageController.createLanguage);
}

export default languageRoutes;
