import {Component, OnInit} from '@angular/core';
import {AbstractControl, FormGroup} from "@angular/forms";

@Component({
  template: ''
})
export abstract class BaseFormComponent implements OnInit {
  //form model
  form!: FormGroup;

  constructor() {
  }

  getErrors(
    control: AbstractControl,
    displayName: string,
  ): string[] {
    let errors: string[] = [];
    Object.keys(control.errors || {}).forEach(key => {
      switch (key) {
        case 'required':
          errors.push(`${displayName} is required`);
          break;
        case 'pattern':
          errors.push(`${displayName} must be a valid name`);
          break;
        case 'isDupeField':
          errors.push(`${displayName} is already in use`);
          break;
        default:
          errors.push(`${displayName} is invalid`);
          break;
      }
    });
    return errors;
  }

  ngOnInit(): void {
  }

}
