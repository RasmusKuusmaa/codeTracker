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

            if (!userData.password) {
                throw new AppError(400, 'Password is required');
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
}

export default new UserService();
