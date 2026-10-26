import { Pipe, PipeTransform } from '@angular/core';

/** Traduce el stock numérico a una etiqueta legible. */
@Pipe({
  name: 'stockStatus',
})
export class StockStatusPipe implements PipeTransform {
  transform(stock: number | null | undefined): string {
    if (stock === null || stock === undefined) {
      return 'Sin datos';
    }

    if (stock <= 0) {
      return 'Sin stock';
    }

    if (stock <= 5) {
      return 'Stock bajo';
    }

    return 'Disponible';
  }
}
