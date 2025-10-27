import userModel from '../models/user.model';
import { UserResponse, CreateUserDTO } from '../types/user.types';
import { AppError } from '../types/common.types';
import { logger } from '../utils/logger';

export class UserService {
    async getAllUsers(): Promise<UserResponse[]> {
        try {
            const users = await userModel.findAll();
            return users.map(user => ({
                user_id: user.user_id,
                username: user.username
            }));
        } catch (error) {
            logger.error('Error in getAllUsers service:', error);
            throw error;
        }
    }

    async registerUser(userData: CreateUserDTO): Promise<UserResponse> {
        try {
            if (!userData.username || userData.username.trim().length === 0) {
                throw new AppError(400, 'Username is required');
            }

            if (!userData.password || userData.password.length < 6) {
                throw new AppError(400, 'Password must be at least 6 characters');
            }

            const existingUser = await userModel.findByUsername(userData.username);
            if (existingUser) {
                throw new AppError(409, 'Username already exists');
            }

            const userId = await userModel.create(userData);

            return {
                user_id: userId,
                username: userData.username
            };
        } catch (error) {
            logger.error('Error in registerUser service:', error);
            throw error;
        }
    }

    async loginUser(credentials: CreateUserDTO): Promise<UserResponse> {
        try {
            if (!credentials.username || credentials.username.trim().length === 0) {
                throw new AppError(400, 'Username is required');
            }

            if (!credentials.password) {
                throw new AppError(400, 'Password is required');
            }

            const user = await userModel.findByUsername(credentials.username);
            if (!user) {
                throw new AppError(401, 'Invalid username or password');
            }

            if (user.password !== credentials.password) {
                throw new AppError(401, 'Invalid username or password');
            }

            return {
                user_id: user.user_id,
                username: user.username
            };
        } catch (error) {
            logger.error('Error in loginUser service:', error);
            throw error;
        }
    }
}

export default new UserService();
