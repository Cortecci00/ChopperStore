import { Skin } from './skin.interface';

export interface TransactionItem {
  id: number;
  transactionId: number;
  skinId: number | null;
  skin: Skin | null;
  skinName: string;
  skinPhotoUrl: string | null;
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
  paymentStatus: 'pending' | 'approved' | 'rejected';
}

export interface CheckoutResult {
  transactionId: number;
  checkoutUrl: string;
}
