import { Directive, ElementRef, HostListener, inject, input } from '@angular/core';

/**
 * Directiva personalizada que resalta la fila o tarjeta al pasar el ratón.
 * Permite personalizar el color con el atributo appHighlight.
 */
@Directive({
  selector: '[appHighlight]',
})
export class HighlightDirective {
  private readonly element = inject<ElementRef<HTMLElement>>(ElementRef);

  readonly appHighlight = input<string>('#eef2ff');

  @HostListener('mouseenter')
  onMouseEnter(): void {
    this.element.nativeElement.style.backgroundColor = this.appHighlight();
  }

  @HostListener('mouseleave')
  onMouseLeave(): void {
    this.element.nativeElement.style.backgroundColor = '';
  }
}
