import projectModel from "@models/project.model";
import { Project, CreateProjectDTO, UpdateProjectDTO } from "../types/project.types";
import { logger } from "@utils/logger";
import { AppError } from "../types/common.types";

export class ProjectService {
    async getAllProjects(userId: number): Promise<Project[]> {
        try {
            const projects = await projectModel.findAllProjects(userId);
            return projects.map(project => ({
                project_id: project.project_id,
                user_id: project.user_id,
                name: project.name,
                description: project.description,
                total_time_minutes: project.total_time_minutes,
                created_at: project.created_at,
                updated_at: project.updated_at
            }));
        } catch (error) {
            logger.error('Error in getAllProjects service:', error);
            throw error;
        }
    }

    async getProjectById(projectId: number, userId: number): Promise<Project> {
        try {
            const project = await projectModel.findProjectById(projectId, userId);
            if (!project) {
                throw new AppError(404, 'Project not found');
            }
            return {
                project_id: project.project_id,
                user_id: project.user_id,
                name: project.name,
                description: project.description,
                total_time_minutes: project.total_time_minutes,
                created_at: project.created_at,
                updated_at: project.updated_at
            };
        } catch (error) {
            logger.error('Error in getProjectById service:', error);
            throw error;
        }
    }

    async createProject(userId: number, projectData: CreateProjectDTO): Promise<Project> {
        try {
            const projectId = await projectModel.createProject(userId, projectData);
            const project = await projectModel.findProjectById(projectId, userId);
            if (!project) {
                throw new AppError(500, 'Error retrieving created project');
            }
            return {
                project_id: project.project_id,
                user_id: project.user_id,
                name: project.name,
                description: project.description,
                total_time_minutes: project.total_time_minutes,
                created_at: project.created_at,
                updated_at: project.updated_at
            };
        } catch (error) {
            logger.error('Error in createProject service:', error);
            throw error;
        }
    }

    async updateProject(projectId: number, userId: number, projectData: UpdateProjectDTO): Promise<Project> {
        try {
            const updated = await projectModel.updateProject(projectId, userId, projectData);
            if (!updated) {
                throw new AppError(404, 'Project not found or no changes made');
            }
            const project = await projectModel.findProjectById(projectId, userId);
            if (!project) {
                throw new AppError(500, 'Error retrieving updated project');
            }
            return {
                project_id: project.project_id,
                user_id: project.user_id,
                name: project.name,
                description: project.description,
                total_time_minutes: project.total_time_minutes,
                created_at: project.created_at,
                updated_at: project.updated_at
            };
        } catch (error) {
            logger.error('Error in updateProject service:', error);
            throw error;
        }
    }

    async deleteProject(projectId: number, userId: number): Promise<void> {
        try {
            const deleted = await projectModel.deleteProject(projectId, userId);
            if (!deleted) {
                throw new AppError(404, 'Project not found');
            }
        } catch (error) {
            logger.error('Error in deleteProject service:', error);
            throw error;
        }
    }
}

export default new ProjectService();
