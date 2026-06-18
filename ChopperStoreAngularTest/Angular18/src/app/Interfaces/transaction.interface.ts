import { Skin } from './skin.interface';

export interface TransactionItem {
  id: number;
  transactionId: number;
  skinId: number;
  skin: Skin;
  quantity: number;
  unitPriceAtPurchase: number;
}

export interface Transaction {
  id: number;
  userId: number;
  user: null;
  items: TransactionItem[];
  totalPrice: number;
  transactionDate: string;
}
