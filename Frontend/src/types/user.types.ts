import { RowDataPacket } from 'mysql2';

export interface User {
    user_id: number;
    username: string;
    password: string;
}

export interface UserRow extends RowDataPacket, User {}

export interface CreateUserDTO {
    username: string;
    password: string;
}

export interface UpdateUserDTO {
    username?: string;
    password?: string;
}

export interface UserResponse {
    user_id: number;
    username: string;
}
