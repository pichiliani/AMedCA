#include "mbed.h"
 
AnalogIn analog_value(A5);
 
int main() {
    float meas;
    
    while(1) {
        // Melhor configuração: placa com 100ms e gráfico com 50ms
        // Em geral, sensor do arduíno lendo no dobro do tempo do computador (serial)
        
		// A linha abaixo lê o valor da entrada A5 analógica A%
		// O valor lido é entre 0.0 e 1.0
		
        meas = analog_value.read(); 
        
		// Imprime o resultado na porta serial. Um por linha
        printf("%f\n", meas);

		// Espera 100 ms para ler o valor do sensor novamente
        wait(0.1); 
    }
}
