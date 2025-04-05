import { User } from './user.interface';

export interface UsersResponse {
  isSuccess: boolean;
  result: User[];
  message: string;
}
