import { User } from './user.interface';

export interface EliminarResponse {
  isSuccess: boolean;
  result: User;
  message: string;
}
