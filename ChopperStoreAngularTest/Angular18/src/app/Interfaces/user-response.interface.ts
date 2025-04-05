import { User } from './user.interface';

export interface UserResponse {
  isSuccess: boolean;
  result: User;
  message: string;
}
