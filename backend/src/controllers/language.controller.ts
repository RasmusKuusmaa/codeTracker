import languageService from "@services/language.service";
import { ApiResponse } from "../types/common.types";
import { logger } from "@utils/logger";
import { FastifyReply, FastifyRequest } from "fastify";

export class LanguageController {
    async getAllLanguages(request: FastifyRequest, reply: FastifyReply): Promise<void> {
        try {
            const languages = await languageService.getAllLanguages();

            const response: ApiResponse = {
                success: true,
                data: languages,
                message: 'Languages retrieved successfully'
            };
            reply.code(200).send(response);
        } catch (error: any) {
            logger.error('Error in getAllLanguages controller:', error);
            reply.code(error.statusCode || 500).send({
                success: false,
                error: error.message || 'Internal server error'
            });
        }
    }

    async getLanguageById(request: FastifyRequest<{ Params: { id: string } }>, reply: FastifyReply): Promise<void> {
        try {
            const languageId = parseInt(request.params.id);
            const language = await languageService.getLanguageById(languageId);

            const response: ApiResponse = {
                success: true,
                data: language,
                message: 'Language retrieved successfully'
            };
            reply.code(200).send(response);
        } catch (error: any) {
            logger.error('Error in getLanguageById controller:', error);
            reply.code(error.statusCode || 500).send({
                success: false,
                error: error.message || 'Internal server error'
            });
        }
    }

    async createLanguage(request: FastifyRequest<{ Body: { name: string } }>, reply: FastifyReply): Promise<void> {
        try {
            const { name } = request.body;
            const language = await languageService.createLanguage(name);

            const response: ApiResponse = {
                success: true,
                data: language,
                message: 'Language created successfully'
            };
            reply.code(201).send(response);
        } catch (error: any) {
            logger.error('Error in createLanguage controller:', error);
            reply.code(error.statusCode || 500).send({
                success: false,
                error: error.message || 'Internal server error'
            });
        }
    }

    async getLanguageStats(request: FastifyRequest, reply: FastifyReply): Promise<void> {
        try {
            const userId = request.user!.user_id;
            const stats = await languageService.getLanguageStats(userId);

            const response: ApiResponse = {
                success: true,
                data: stats,
                message: 'Language stats retrieved successfully'
            };
            reply.code(200).send(response);
        } catch (error: any) {
            logger.error('Error in getLanguageStats controller:', error);
            reply.code(error.statusCode || 500).send({
                success: false,
                error: error.message || 'Internal server error'
            });
        }
    }
}

export default new LanguageController();
