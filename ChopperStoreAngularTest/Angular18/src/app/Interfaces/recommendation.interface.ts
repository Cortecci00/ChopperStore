import { User } from './user.interface';

export interface Recommendation {
  id: number;
  usuario: User;
  text: string;
}
