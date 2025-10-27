import userModel from '../models/user.model';
import { UserResponse } from '../types/user.types';
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
}

export default new UserService();
