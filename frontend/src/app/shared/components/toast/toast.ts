import { Component, inject } from '@angular/core';
import { ToastService } from '../../../core/services/toast.service';

@Component({
  selector: 'app-toast',
  template: `
    <div class="toast-stack" aria-live="polite">
      @for (toast of toastService.toasts(); track toast.id) {
        <div class="toast" [class]="'toast toast--' + toast.type">
          <span class="toast__icon" aria-hidden="true">
            @switch (toast.type) {
              @case ('success') {
                ✓
              }
              @case ('error') {
                !
              }
              @default {
                i
              }
            }
          </span>
          <p>{{ toast.message }}</p>
          <button type="button" class="toast__close" (click)="toastService.dismiss(toast.id)" aria-label="Cerrar">
            ×
          </button>
        </div>
      }
    </div>
  `,
  styles: `
    .toast-stack {
      position: fixed;
      right: 1.25rem;
      bottom: 1.25rem;
      z-index: 999;
      display: flex;
      flex-direction: column;
      gap: 0.65rem;
      max-width: min(24rem, calc(100vw - 2rem));
    }

    .toast {
      display: flex;
      align-items: flex-start;
      gap: 0.65rem;
      padding: 0.85rem 1rem;
      border-radius: 0.85rem;
      background: #ffffff;
      color: #1e293b;
      box-shadow: 0 18px 40px rgba(15, 23, 42, 0.18);
      border-left: 4px solid #4f46e5;
      animation: slide-in 0.2s ease-out;
    }

    .toast p {
      flex: 1;
      margin: 0;
      font-size: 0.875rem;
      line-height: 1.4;
    }

    .toast__icon {
      display: grid;
      place-items: center;
      width: 1.4rem;
      height: 1.4rem;
      border-radius: 50%;
      font-size: 0.8rem;
      font-weight: 700;
      color: #ffffff;
      background: #4f46e5;
      flex-shrink: 0;
    }

    .toast--success {
      border-left-color: #10b981;
    }

    .toast--success .toast__icon {
      background: #10b981;
    }

    .toast--error {
      border-left-color: #ef4444;
    }

    .toast--error .toast__icon {
      background: #ef4444;
    }

    .toast__close {
      border: none;
      background: transparent;
      color: #94a3b8;
      font-size: 1.1rem;
      line-height: 1;
      cursor: pointer;
      padding: 0;
    }

    .toast__close:hover {
      color: #0f172a;
    }

    @keyframes slide-in {
      from {
        opacity: 0;
        transform: translateY(0.5rem);
      }
      to {
        opacity: 1;
        transform: translateY(0);
      }
    }
  `,
})
export class ToastComponent {
  protected readonly toastService = inject(ToastService);
}
