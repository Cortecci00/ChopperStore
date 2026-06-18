export interface ContactMessage {
  id: number;
  name: string;
  lastname: string;
  email: string;
  message: string;
  createdAt: string;
}

export interface ContactMessageCreate {
  name: string;
  lastname: string;
  email: string;
  message: string;
}
