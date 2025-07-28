// src/app/products/add-product/add-product.component.ts

import { Component, EventEmitter, Input, Output, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators, ReactiveFormsModule, AbstractControl, ValidatorFn, ValidationErrors, FormsModule } from '@angular/forms';
import { CommonModule, AsyncPipe } from '@angular/common';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { finalize, map } from 'rxjs';
import { Observable } from 'rxjs';
import { CategoryService } from '../../../../services/library-management/category.service';
import { ProductService } from '../../../../services/library-management/product.service';
import { AlterComponent } from '../../../../shared/components/alter/alter.component';
import { LoaderComponent } from '../../../../shared/components/loader/loader.component';
import { CategoryLookUpDto } from '../../../../shared/models/library-management/categories/category-lookup.dto';
import { ProductCreateRequestDto } from '../../../../shared/models/library-management/products/product-create-request.dto';


@Component({
  selector: 'app-add-product',
  standalone: true,
  imports: [
    TranslatePipe,
    CommonModule,
    ReactiveFormsModule,
    FormsModule,
    LoaderComponent,
    AlterComponent
  ],
  templateUrl: './add-product.component.html',
  styleUrls: ['./add-product.component.css']
})
export class AddProductComponent implements OnInit {
  addForm!: FormGroup;
  isLoadingResult: boolean = false;
  isSubmitted: boolean = false;

  alertVisible = false;
  alertMessage = '';
  isSuccessAlert = false;

  categories$!: Observable<CategoryLookUpDto[]>;
  allCategories: CategoryLookUpDto[] = [];
  selectedCategoriesForDisplay: CategoryLookUpDto[] = []; // لتخزين كائنات التصنيف المختارة للعرض (معرف واسم)

  selectedCategoryIdToAdd: string = ''; // لتخزين معرف التصنيف المختار من الـ select المفرد

  @Output() visibleChange = new EventEmitter<boolean>();
  @Input() visible = false;
  @Output() productAdded = new EventEmitter<void>();

  selectedImageFile: File | null = null;
  imageUrlPreview: string | ArrayBuffer | null = null;

  constructor(
    private productService: ProductService,
    private categoryService: CategoryService,
    private translate: TranslateService,
    private fb: FormBuilder
  ) { }

  ngOnInit() {
    this.initializeForm();
    this.loadCategories();
  }

  private initializeForm(): void {
    this.addForm = this.fb.group({
      arProductName: ['', [Validators.required, Validators.maxLength(255)]],
      arProductDescription: ['', [Validators.required, Validators.maxLength(1000)]],
      enProductName: ['', [Validators.required, Validators.maxLength(255)]],
      enProductDescription: ['', [Validators.required, Validators.maxLength(1000)]],
      productPrice: ['', [Validators.required, Validators.min(0.01)]],
      productStock: ['', [Validators.required, Validators.min(0), Validators.pattern(/^[0-9]*$/)]],
      categoriesIds: [[] as string[], this.minSelectedCheck(1)], // لا يزال مصفوفة من الـ IDs
      imageFile: [null, Validators.required]
    });
  }

  // مُدقق مخصص للتأكد من اختيار عنصر واحد على الأقل في مصفوفة
  minSelectedCheck(min: number): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
      const value = control.value;
      if (Array.isArray(value) && value.length >= min) {
        return null; // صالح
      }
      return { minSelected: { requiredMin: min, actual: value ? value.length : 0 } }; // غير صالح
    };
  }

  private loadCategories(): void {
    this.categories$ = this.categoryService.getCategoryLookup(0).pipe(
      map(categories => {
        this.allCategories = categories;
        return categories;
      })
    );
  }

  // دالة لإضافة التصنيف المختار من الـ select العادي
  addCategory(): void {
    if (this.selectedCategoryIdToAdd && this.selectedCategoryIdToAdd !== '') {
      const categoryToAdd = this.allCategories.find(cat => cat.categoryId === this.selectedCategoryIdToAdd);
      if (categoryToAdd && !this.selectedCategoriesForDisplay.some(c => c.categoryId === categoryToAdd.categoryId)) {
        // تحديث selectedCategoriesForDisplay بطريقة تضمن اكتشاف التغيير
        this.selectedCategoriesForDisplay = [...this.selectedCategoriesForDisplay, categoryToAdd]; // <--- التعديل هنا

        // تحديث قيمة categoriesIds في الفورم
        const currentIds = this.addForm.get('categoriesIds')?.value as string[];
        this.addForm.get('categoriesIds')?.setValue([...currentIds, categoryToAdd.categoryId]);
        this.addForm.get('categoriesIds')?.markAsDirty(); // وضع الحقل كـ dirty لتشغيل الـ validation
        this.addForm.get('categoriesIds')?.updateValueAndValidity(); // تحديث حالة الـ validation

        this.selectedCategoryIdToAdd = ''; // إعادة تعيين الـ select
      }
    }
  }

  // دالة لإزالة التصنيف من القائمة المختارة
  removeCategory(categoryId: string): void {
    // تحديث selectedCategoriesForDisplay بطريقة تضمن اكتشاف التغيير
    this.selectedCategoriesForDisplay = this.selectedCategoriesForDisplay.filter(cat => cat.categoryId !== categoryId); // <--- التأكد من هذا أيضاً
    // تحديث قيمة categoriesIds في الفورم بعد الحذف
    const currentIds = this.addForm.get('categoriesIds')?.value as string[];
    this.addForm.get('categoriesIds')?.setValue(currentIds.filter(id => id !== categoryId));
    this.addForm.get('categoriesIds')?.markAsDirty(); // وضع الحقل كـ dirty لتشغيل الـ validation
    this.addForm.get('categoriesIds')?.updateValueAndValidity(); // تحديث حالة الـ validation
  }


  onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files.length > 0) {
      this.selectedImageFile = input.files[0];
      this.addForm.patchValue({ imageFile: this.selectedImageFile });
      this.addForm.get('imageFile')?.updateValueAndValidity();

      const reader = new FileReader();
      reader.onload = () => {
        this.imageUrlPreview = reader.result;
      };
      reader.readAsDataURL(this.selectedImageFile);
    } else {
      this.selectedImageFile = null;
      this.imageUrlPreview = null;
      this.addForm.patchValue({ imageFile: null });
      this.addForm.get('imageFile')?.updateValueAndValidity();
    }
  }

  onSubmit(): void {
    this.isSubmitted = true;
    this.addForm.markAllAsTouched();

    if (this.addForm.invalid) {
      console.log('Form is invalid. Not submitting.');
      this.showAlertMessage(false, 'VALIDATION.FORM_INVALID_ERROR');
      return;
    }

    const request: ProductCreateRequestDto = {
      aRProductName: this.addForm.value.arProductName,
      aRProductDescription: this.addForm.value.arProductDescription,
      eNProductName: this.addForm.value.enProductName,
      eNProductDescription: this.addForm.value.enProductDescription,
      productPrice: +this.addForm.value.productPrice,
      productStock: +this.addForm.value.productStock,
      categoriesIds: this.addForm.get('categoriesIds')?.value, // استخدم القيمة المباشرة من الفورم
      imageFile: this.selectedImageFile
    };

    this.isLoadingResult = true;
    console.log('Submitting product creation request...', request);

    this.productService.addProduct(request)
      .pipe(
        finalize(() => {
          this.isLoadingResult = false;
          console.log('API call finalized. isLoadingResult set to false.');
        })
      )
      .subscribe({
        next: response => {
          this.isSuccessAlert = response.isSuccess;
          console.log('API Response received. isSuccessAlert:', this.isSuccessAlert, 'Status Key:', response.statusKey);

          this.translate.get(response.statusKey || (response.isSuccess ? 'PRODUCTS.ADD_SUCCESS' : 'PRODUCTS.ADD_FAILED'))
            .subscribe(translatedMessage => {
              this.alertMessage = translatedMessage;
              this.alertVisible = true;
              console.log('Alert message set. alertVisible set to true.');
            });
          if (response.isSuccess) {
            this.addForm.reset();
            this.isSubmitted = false;
            this.selectedImageFile = null;
            this.imageUrlPreview = null;
            this.selectedCategoriesForDisplay = []; // مسح التصنيفات المعروضة
            this.selectedCategoryIdToAdd = ''; // مسح اختيار الـ select المفرد
            this.productAdded.emit();
            console.log('Product added successfully. productAdded event emitted.');
          }
        },
        error: error => {
          this.isSuccessAlert = false;
          console.error('API Error:', error);
          this.translate.get(error.error?.statusKey || error.message || 'COMMON.SERVER_ERROR')
            .subscribe(translatedMessage => {
              this.alertMessage = translatedMessage;
              this.alertVisible = true;
              console.log('Error alert message set. alertVisible set to true.');
            });
        }
      });
  }

  onAlertClosed(): void {
    this.alertVisible = false;
    if (this.isSuccessAlert) {
      this.closeModal();
    }
  }

  closeModal(): void {
    this.addForm.reset();
    this.isSubmitted = false;
    this.selectedImageFile = null;
    this.imageUrlPreview = null;
    this.selectedCategoriesForDisplay = []; // مسح التصنيفات المعروضة
    this.selectedCategoryIdToAdd = ''; // مسح اختيار الـ select المفرد
    this.visibleChange.emit(false);
  }

  getControl(name: string): AbstractControl | null {
    return this.addForm.get(name);
  }

  showError(controlName: string): boolean {
    const control = this.getControl(controlName);
    return (control?.invalid && (control?.dirty || control?.touched || this.isSubmitted)) ?? false;
  }

  private showAlertMessage(isSuccess: boolean, messageKey: string): void {
    this.isSuccessAlert = isSuccess;
    this.alertMessage = messageKey;
    this.alertVisible = true;
  }
}
