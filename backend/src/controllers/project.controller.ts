import projectService from "@services/project.service";
import { ApiResponse } from "../types/common.types";
import { logger } from "@utils/logger";
import { FastifyReply, FastifyRequest } from "fastify";
import { CreateProjectDTO, UpdateProjectDTO } from "../types/project.types";

export class ProjectController {
    async getAllProjects(request: FastifyRequest, reply: FastifyReply): Promise<void> {
        try {
            const userId = request.user!.user_id;
            const projects = await projectService.getAllProjects(userId);

            const response: ApiResponse = {
                success: true,
                data: projects,
                message: 'Projects retrieved successfully'
            };
            reply.code(200).send(response);
        } catch (error: any) {
            logger.error('Error in getAllProjects controller:', error);
            reply.code(error.statusCode || 500).send({
                success: false,
                error: error.message || 'Internal server error'
            });
        }
    }

    async getProjectById(request: FastifyRequest<{ Params: { id: string } }>, reply: FastifyReply): Promise<void> {
        try {
            const userId = request.user!.user_id;
            const projectId = parseInt(request.params.id);

            if (isNaN(projectId)) {
                reply.code(400).send({
                    success: false,
                    error: 'Invalid project ID'
                });
                return;
            }

            const project = await projectService.getProjectById(projectId, userId);

            const response: ApiResponse = {
                success: true,
                data: project,
                message: 'Project retrieved successfully'
            };
            reply.code(200).send(response);
        } catch (error: any) {
            logger.error('Error in getProjectById controller:', error);
            reply.code(error.statusCode || 500).send({
                success: false,
                error: error.message || 'Internal server error'
            });
        }
    }

    async createProject(request: FastifyRequest<{ Body: CreateProjectDTO }>, reply: FastifyReply): Promise<void> {
        try {
            const userId = request.user!.user_id;
            const projectData = request.body;

            const project = await projectService.createProject(userId, projectData);

            const response: ApiResponse = {
                success: true,
                data: project,
                message: 'Project created successfully'
            };
            reply.code(201).send(response);
        } catch (error: any) {
            logger.error('Error in createProject controller:', error);
            reply.code(error.statusCode || 500).send({
                success: false,
                error: error.message || 'Internal server error'
            });
        }
    }

    async updateProject(request: FastifyRequest<{ Params: { id: string }, Body: UpdateProjectDTO }>, reply: FastifyReply): Promise<void> {
        try {
            const userId = request.user!.user_id;
            const projectId = parseInt(request.params.id);
            const projectData = request.body;

            if (isNaN(projectId)) {
                reply.code(400).send({
                    success: false,
                    error: 'Invalid project ID'
                });
                return;
            }

            const project = await projectService.updateProject(projectId, userId, projectData);

            const response: ApiResponse = {
                success: true,
                data: project,
                message: 'Project updated successfully'
            };
            reply.code(200).send(response);
        } catch (error: any) {
            logger.error('Error in updateProject controller:', error);
            reply.code(error.statusCode || 500).send({
                success: false,
                error: error.message || 'Internal server error'
            });
        }
    }

    async deleteProject(request: FastifyRequest<{ Params: { id: string } }>, reply: FastifyReply): Promise<void> {
        try {
            const userId = request.user!.user_id;
            const projectId = parseInt(request.params.id);

            if (isNaN(projectId)) {
                reply.code(400).send({
                    success: false,
                    error: 'Invalid project ID'
                });
                return;
            }

            await projectService.deleteProject(projectId, userId);

            const response: ApiResponse = {
                success: true,
                data: null,
                message: 'Project deleted successfully'
            };
            reply.code(200).send(response);
        } catch (error: any) {
            logger.error('Error in deleteProject controller:', error);
            reply.code(error.statusCode || 500).send({
                success: false,
                error: error.message || 'Internal server error'
            });
        }
    }
}

export default new ProjectController();
