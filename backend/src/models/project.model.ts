import database from "@config/database";
import { AppError } from "../types/common.types";
import { ProjectRow, CreateProjectDTO, UpdateProjectDTO } from "../types/project.types";

export class ProjectModel {
    private pool = database.getPool();

    async findAllProjects(userId: number): Promise<ProjectRow[]> {
        try {
            const [rows] = await this.pool.query<ProjectRow[]>(
                `SELECT project_id, user_id, name, description, total_time_minutes, created_at, updated_at
                FROM projects
                WHERE user_id = ?`,
                [userId]
            );
            return rows;
        } catch (error) {
            throw new AppError(500, 'Error fetching projects from database');
        }
    }

    async findProjectById(projectId: number, userId: number): Promise<ProjectRow | null> {
        try {
            const [rows] = await this.pool.query<ProjectRow[]>(
                `SELECT project_id, user_id, name, description, total_time_minutes, created_at, updated_at
                FROM projects
                WHERE project_id = ? AND user_id = ?`,
                [projectId, userId]
            );
            return rows[0] || null;
        } catch (error) {
            throw new AppError(500, 'Error fetching project from database');
        }
    }

    async createProject(userId: number, projectData: CreateProjectDTO): Promise<number> {
        try {
            const [result] = await this.pool.query(
                `INSERT INTO projects (user_id, name, description) VALUES (?, ?, ?)`,
                [userId, projectData.name, projectData.description || null]
            );
            return (result as any).insertId;
        } catch (error) {
            throw new AppError(500, 'Error creating project in database');
        }
    }

    async updateProject(projectId: number, userId: number, projectData: UpdateProjectDTO): Promise<boolean> {
        try {
            const fields: string[] = [];
            const values: any[] = [];

            if (projectData.name !== undefined) {
                fields.push('name = ?');
                values.push(projectData.name);
            }
            if (projectData.description !== undefined) {
                fields.push('description = ?');
                values.push(projectData.description);
            }

            if (fields.length === 0) {
                return false;
            }

            values.push(projectId, userId);

            const [result] = await this.pool.query(
                `UPDATE projects SET ${fields.join(', ')} WHERE project_id = ? AND user_id = ?`,
                values
            );

            return (result as any).affectedRows > 0;
        } catch (error) {
            throw new AppError(500, 'Error updating project in database');
        }
    }

    async deleteProject(projectId: number, userId: number): Promise<boolean> {
        try {
            const [result] = await this.pool.query(
                `DELETE FROM projects WHERE project_id = ? AND user_id = ?`,
                [projectId, userId]
            );
            return (result as any).affectedRows > 0;
        } catch (error) {
            throw new AppError(500, 'Error deleting project from database');
        }
    }
}

export default new ProjectModel();
