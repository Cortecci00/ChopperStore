export interface User {
  id: number;
  name: string;
  lastname: string;
  email: string;
  phone: string;
  username: string;
  password?: string;
  isAdmin: boolean;
  isBlocked: boolean;
  photoUrl?: string | null;
  steamTradeUrl?: string | null;
}
