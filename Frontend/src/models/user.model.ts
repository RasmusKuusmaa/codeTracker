import database from '../config/database';
import { UserRow } from '../types/user.types';
import { AppError } from '../types/common.types';

export class UserModel {
    private pool = database.getPool();

    async findAll(): Promise<UserRow[]> {
        try {
            const [rows] = await this.pool.query<UserRow[]>(
                'SELECT user_id, username, password FROM users'
            );
            return rows;
        } catch (error) {
            throw new AppError(500, 'Error fetching users from database');
        }
    }
}

export default new UserModel();
