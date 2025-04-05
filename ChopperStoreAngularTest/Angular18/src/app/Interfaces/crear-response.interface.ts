import { User } from './user.interface';

export interface CrearResponse {
  isSuccess: boolean;
  result: User;
  message: string;
}
