import { Component, EventEmitter, Input, OnDestroy, OnInit, Output } from '@angular/core';
import { QigUserModel } from 'src/app/model/project/qig';
import { AlertService } from 'src/app/services/common/alert.service';
import { CommonService } from 'src/app/services/common/common.service';
import { QigService } from 'src/app/services/project/qig.service';
import { ActivatedRoute } from '@angular/router';
import { OwlOptions } from 'ngx-owl-carousel-o';

@Component({
  selector: 'emarking-qig-tab',
  templateUrl: './qig-tab.component.html',
  styleUrls: ['./qig-tab.component.css']
})
export class QigTabComponent implements OnInit, OnDestroy {

  constructor(public qigService: QigService, public Alert: AlertService,
    private commonservice: CommonService, private router: ActivatedRoute) { }
  ngOnDestroy(): void {
    this.commonservice.IsKp = false;
  }

  ngOnInit(): void {
    this.getAllQigs();
  }
  stasticsloading: boolean = false;
  Qigs?: QigUserModel[];
  selectedQig?: QigUserModel;
  @Input() DefaultTabName?: string;
  @Input() OnlyIsKpTab?: boolean;
  @Output() selectQigEvent = new EventEmitter<QigUserModel | undefined>();

  getAllQigs() {
    this.stasticsloading = true;
    this.qigService.getQigs(this.OnlyIsKpTab).subscribe(data => {  
       if (data == null || data == undefined) {
       if (this.DefaultTabName != null && this.DefaultTabName != undefined && this.DefaultTabName != "") {
          data = [];
        }
      }     
      if (data != null && data != undefined) {
        data.sort((a, b) => a.QigCode.localeCompare(b.QigCode))
        this.Qigs = data;
        if (this.DefaultTabName != null && this.DefaultTabName != undefined && this.DefaultTabName != "") {
          let alluser = new QigUserModel();
          alluser.IsKp = false;
          alluser.IsS1Available = true;
          alluser.QigName = this.DefaultTabName;
          alluser.QigId = 0;
          alluser.QigCode = this.DefaultTabName;
          data.unshift(alluser);
        }
        if (data.length > 0) {
          var qids = this.router.snapshot.params['qigid'] || this.router.snapshot.params['QigId'];
          let jqid = this.Qigs.find(a => a.QigId == qids);
          this.customOptions.startPosition = this.Qigs.findIndex(a => a.QigId == qids)>-1 ? 
          this.Qigs.findIndex(a => a.QigId == qids):0;
          if (qids != null && qids != undefined && jqid != undefined && jqid != null) {
            this.selectQigItem(jqid);
          } else {
            this.selectQigItem(this.Qigs[0]);
          }
        }
      }
    }, (err: any) => {
      throw (err)
    }, () => {
      this.stasticsloading = false;
    });
  }

  selectQigItem(value: QigUserModel) {
    this.selectedQig = value;
    this.commonservice.IsKp = value.IsKp;
    this.selectQigEvent.emit(value);
  }
  trackByMethod(el: QigUserModel): number {
    return el.QigId;
  }

  customOptions: OwlOptions = {
    loop: false,
    mouseDrag: true,
    touchDrag: false,
    pullDrag: false,
    dots: false,
    navSpeed: 700,
    navText: ['<span _ngcontent-lpb-c132="" class="icon-arrow_back" title="Previous"></span>', '<span title="Next" _ngcontent-lpb-c132="" class="icon-arrow_back"></span>'],
    responsive: {
      0: {
        items: 1
      },
      400: {
        items: 2
      },
      740: {
        items: 3
      },
      940: {
        items: 5
      }
    },
    nav: true
  }
}
