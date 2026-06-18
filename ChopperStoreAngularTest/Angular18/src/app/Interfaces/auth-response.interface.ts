import { User } from './user.interface';

export interface AuthResponse {
  isSuccess: boolean;
  result: {
    user: User;
    token: string;
  };
  message: string;
}
