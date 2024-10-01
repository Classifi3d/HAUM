import { inject } from '@angular/core';
import {
  ActivatedRouteSnapshot,
  CanActivateFn,
  Router,
  RouterStateSnapshot,
  UrlTree,
} from '@angular/router';
import { Observable } from 'rxjs';

export const authGuard: CanActivateFn = (
  route: ActivatedRouteSnapshot,
  state: RouterStateSnapshot
):
  | Observable<boolean | UrlTree>
  | Promise<boolean | UrlTree>
  | boolean
  | UrlTree => {
  let token = localStorage.getItem('guid');
  // if (
  //   token == 'non-valid token' ||
  //   token == '' ||
  //   token == null ||
  //   token == undefined
  // ) {
  //   return inject(Router).createUrlTree(['/']);
  // } else {
  return true;
  // }
};
