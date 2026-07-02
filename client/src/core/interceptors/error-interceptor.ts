import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError } from 'rxjs';
import { ToastService } from '../services/toast-service';
import { Router, NavigationExtras } from '@angular/router';

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const toast = inject(ToastService);
  const router = inject(Router);

  return next(req).pipe(
    catchError(error => {
      if (error) {
        const errorData = error.error;
        switch(error.status) {
          case 400:
            if (errorData?.errors) {
              const modelStateErrors = [];
              for (const key in errorData?.errors) {
                if (errorData?.errors[key]) {
                  modelStateErrors.push(errorData?.errors[key]);
                }
              }
              throw modelStateErrors.flat();
            }else {
              toast.error(errorData);
            }
            break;
          case 401:
            toast.error('Unauthorized');
            break;
          case 404:
            router.navigateByUrl('/not-found');
            break;
          case 500:
            const navigationExtras: NavigationExtras = { state: { error: errorData } };
            router.navigate(['/server-error'], navigationExtras);
            break;
          default:
            toast.error('Something went Wrong');
            break;
        }
      }
      throw error;
    })
  );
};
