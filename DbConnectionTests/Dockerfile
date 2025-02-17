FROM mysql:8.4.4

RUN sed -i '/^\[mysqld\]/a mysql_native_password=ON' /etc/my.cnf

CMD ["mysqld", "--mysql-native-password=ON"]