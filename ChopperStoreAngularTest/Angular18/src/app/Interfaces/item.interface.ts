import { Skin } from './skin.interface';

export interface Item {
  id: number;
  skinId: number;
  skin: Skin;
  quantity: number;
  shoppingCartId: number;
}
