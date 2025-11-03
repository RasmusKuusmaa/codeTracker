import { RowDataPacket } from 'mysql2';

export interface Project {
    project_id: number;
    user_id: number;
    name: string;
    description?: string | null;
    total_time_minutes: number;
    created_at?: string;
    updated_at?: string;
}

export interface ProjectRow extends RowDataPacket, Project {}

export interface CreateProjectDTO {
    name: string;
    description?: string;
}

export interface UpdateProjectDTO {
    name?: string;
    description?: string;
}
