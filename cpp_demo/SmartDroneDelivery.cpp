#include <iostream>
#include <string>
using namespace std;

int main() {
    string maDon, nguoiNhan, diaChi;
    int trangThai;

    cout << "===== SMART DRONE DELIVERY =====\n";

    cout << "Nhap ma don hang: ";
    cin >> maDon;

    cin.ignore();

    cout << "Nhap ten nguoi nhan: ";
    getline(cin, nguoiNhan);

    cout << "Nhap dia chi giao hang: ";
    getline(cin, diaChi);

    cout << "Chon trang thai don hang:\n";
    cout << "1. Cho lay hang\n";
    cout << "2. Dang van chuyen\n";
    cout << "3. Da giao hang\n";
    cout << "Nhap lua chon: ";
    cin >> trangThai;

    cout << "\n===== THONG TIN DON HANG =====\n";
    cout << "Ma don: " << maDon << endl;
    cout << "Nguoi nhan: " << nguoiNhan << endl;
    cout << "Dia chi: " << diaChi << endl;

    if (trangThai == 1)
        cout << "Trang thai: Cho lay hang";
    else if (trangThai == 2)
        cout << "Trang thai: Dang van chuyen";
    else if (trangThai == 3)
        cout << "Trang thai: Da giao hang";
    else
        cout << "Trang thai khong hop le";

    return 0;
}
