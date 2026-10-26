import { Component, computed, inject, signal } from '@angular/core';
import { RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { AuthService } from '../core/services/auth.service';
import { ToastComponent } from '../shared/components/toast/toast';

@Component({
  selector: 'app-shell',
  imports: [RouterOutlet, RouterLink, RouterLinkActive, ToastComponent],
  templateUrl: './shell.html',
  styleUrl: './shell.scss',
})
export class Shell {
  private readonly authService = inject(AuthService);

  protected readonly menuOpen = signal(false);
  protected readonly usuario = this.authService.usuario;
  protected readonly isAdmin = this.authService.isAdmin;
  protected readonly iniciales = computed(() => {
    const nombre = this.usuario()?.nombreCompleto?.trim();
    if (!nombre) {
      return 'TW';
    }

    return nombre
      .split(/\s+/)
      .slice(0, 2)
      .map((parte) => parte.charAt(0).toUpperCase())
      .join('');
  });

  protected logout(): void {
    this.menuOpen.set(false);
    this.authService.logout();
  }

  protected closeMenu(): void {
    this.menuOpen.set(false);
  }
}
