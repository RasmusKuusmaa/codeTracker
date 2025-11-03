import languageModel from "@models/language.model";
import { Language } from "../types/csession.types";
import { logger } from "@utils/logger";
import { AppError } from "../types/common.types";

export class LanguageService {
    async getAllLanguages(): Promise<Language[]> {
        try {
            return await languageModel.findAllLanguages();
        } catch (error) {
            logger.error('Error in getAllLanguages service:', error);
            throw error;
        }
    }

    async getLanguageById(languageId: number): Promise<Language> {
        try {
            const language = await languageModel.findLanguageById(languageId);
            if (!language) {
                throw new AppError(404, 'Language not found');
            }
            return language;
        } catch (error) {
            logger.error('Error in getLanguageById service:', error);
            throw error;
        }
    }

    async createLanguage(name: string): Promise<Language> {
        try {
            const languageId = await languageModel.createLanguage(name);
            const language = await languageModel.findLanguageById(languageId);
            if (!language) {
                throw new AppError(500, 'Error retrieving created language');
            }
            return language;
        } catch (error) {
            logger.error('Error in createLanguage service:', error);
            throw error;
        }
    }

    async getLanguageStats(userId: number): Promise<any[]> {
        try {
            return await languageModel.getLanguageStats(userId);
        } catch (error) {
            logger.error('Error in getLanguageStats service:', error);
            throw error;
        }
    }
}

export default new LanguageService();
