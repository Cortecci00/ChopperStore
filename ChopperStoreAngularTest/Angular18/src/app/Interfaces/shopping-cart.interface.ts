import { Item } from './item.interface';

export interface ShoppingCart {
  id: number;
  userId: number;
  user: null;
  items: Item[];
}
