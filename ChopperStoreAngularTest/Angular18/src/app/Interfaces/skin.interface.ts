import { Category } from './category.interface';

export interface Skin {
  id: number;
  name: string;
  skinFloat: number;
  pattern: number;
  rarity: string;
  price: number;
  category: Category;
  photoUrl?: string | null;
  inspectLink?: string | null;
}

export interface SkinCreateUpdate {
  name: string;
  skinFloat: number;
  pattern: number;
  rarity: string;
  price: number;
  categoryId: number;
  photoUrl?: string | null;
  inspectLink?: string | null;
}
