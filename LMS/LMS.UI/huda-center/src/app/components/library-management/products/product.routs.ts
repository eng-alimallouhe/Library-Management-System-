import { Routes } from "@angular/router";
import { ProductsComponent } from "./products/products.component";

export const PRODUCT_ROUTES : Routes = [
    {
        path: '',
        component: ProductsComponent
    }
];